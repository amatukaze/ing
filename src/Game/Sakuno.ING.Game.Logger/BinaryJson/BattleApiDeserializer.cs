using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sakuno.ING.Game.Json;
using static System.Linq.Expressions.Expression;

namespace Sakuno.ING.Game.Logger.BinaryJson
{
    internal class BattleApiDeserializer
    {
        private readonly Func<BinaryJsonReader, BattleDetailJson> func;

        private static readonly MethodInfo
            startObjectOrSkip, tryReadJName, tryReadContainerLengthOrSkip,
            readIntegerOrSkip, readBooleanOrSkip, readNumberOrSkip, readStringOrSkip, skipValue;
        static BattleApiDeserializer()
        {
            var reader = typeof(BinaryJsonReader);
            startObjectOrSkip = reader.GetMethod(nameof(BinaryJsonReader.StartObjectOrSkip));
            tryReadJName = reader.GetMethod(nameof(BinaryJsonReader.TryReadJName));
            tryReadContainerLengthOrSkip = reader.GetMethod(nameof(BinaryJsonReader.TryReadContainerLengthOrSkip));
            readIntegerOrSkip = reader.GetMethod(nameof(BinaryJsonReader.ReadIntegerOrSkip));
            readBooleanOrSkip = reader.GetMethod(nameof(BinaryJsonReader.ReadBooleanOrSkip));
            readNumberOrSkip = reader.GetMethod(nameof(BinaryJsonReader.ReadNumberOrSkip));
            readStringOrSkip = reader.GetMethod(nameof(BinaryJsonReader.ReadStringOrSkip));
            skipValue = reader.GetMethod(nameof(BinaryJsonReader.SkipValue));
        }

        public BattleApiDeserializer(IReadOnlyDictionary<string, int> resolver)
        {
            var reader = Parameter(typeof(BinaryJsonReader));
            func = Lambda<Func<BinaryJsonReader, BattleDetailJson>>(Invoke(ReadObject(typeof(BattleDetailJson), resolver, reader)), reader).Compile();
        }

        public static Expression ReadValue(Type type, IReadOnlyDictionary<string, int> resolver, ParameterExpression reader)
        {
            if (type == typeof(int))
                return Coalesce(Call(reader, readIntegerOrSkip), Constant(0));
            else if (type == typeof(int?))
                return Call(reader, readIntegerOrSkip);
            else if (type == typeof(bool))
                return Coalesce(Call(reader, readBooleanOrSkip), Constant(false));
            else if (type == typeof(decimal))
                return Coalesce(Call(reader, readNumberOrSkip), Constant(0m));
            else if (type == typeof(string))
                return Call(reader, readStringOrSkip);
            else if (type.IsEnum)
                return Convert(Coalesce(Call(reader, readIntegerOrSkip), Constant(0)), type);
            else if (type.IsArray)
                return Invoke(ReadArray(type, resolver, reader));
            else
                return Invoke(ReadObject(type, resolver, reader));
        }

        public static LambdaExpression ReadObject(Type type, IReadOnlyDictionary<string, int> resolver, ParameterExpression reader)
        {
            var ret = Label(type);
            var result = Variable(type);
            var jName = Variable(typeof(int));
            return Lambda(Block
            (
                IfThen(Call(reader, startObjectOrSkip),
                    Block(new[] { result, jName },
                        Assign(result, New(type)),
                        Loop(IfThenElse(Call(reader, tryReadJName, jName),
                            Switch(typeof(void), jName,
                                Call(reader, skipValue), null,
                                type.GetFields().Select(f =>
                                    SwitchCase(Assign(Field(result, f), ReadValue(f.FieldType, resolver, reader)),
                                        Constant(resolver[f.Name]))).ToArray()),
                            Return(ret, result)))
                    )),
                Label(ret, Constant(null, type))
            ));
        }

        public static LambdaExpression ReadArray(Type arrayType, IReadOnlyDictionary<string, int> resolver, ParameterExpression reader)
        {
            var elementType = arrayType.GetElementType();
            var ret = Label(arrayType);
            var result = Variable(arrayType);
            var length = Variable(typeof(int));
            var index = Variable(typeof(int));
            var readValue = ReadValue(elementType, resolver, reader);
            return Lambda(Block(new[] { length },
                IfThen(Call(reader, tryReadContainerLengthOrSkip, length),
                    Block(new[] { result, index },
                        Assign(result, NewArrayBounds(elementType, length)),
                        Assign(index, Constant(0)),
                        Loop(IfThenElse(LessThan(index, length),
                            Block(Assign(ArrayAccess(result, index), readValue), PostIncrementAssign(index)),
                            Return(ret, result)))
                    )),
                Label(ret, Constant(null, arrayType))
            ));
        }

        public BattleDetailJson Deserialize(ReadOnlyMemory<byte> data) => func(new BinaryJsonReader(data));
    }
}
