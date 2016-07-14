using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class QuestCategoryToBrushConverter : IValueConverter
    {
        static Brush r_Composition, r_Sortie, r_Practice, r_Expedition, r_SupplyOrDocking, r_Arsenal, r_Modernization;

        static QuestCategoryToBrushConverter()
        {
            r_Composition = new SolidColorBrush(Color.FromRgb(0x43, 0xC7, 0x69));
            r_Composition.Freeze();

            r_Sortie = new SolidColorBrush(Color.FromRgb(0xEC, 0x60, 0x63));
            r_Sortie.Freeze();

            r_Practice = new SolidColorBrush(Color.FromRgb(0x93, 0xCE, 0x67));
            r_Practice.Freeze();

            r_Expedition = new SolidColorBrush(Color.FromRgb(0x4E, 0xBB, 0xD4));
            r_Expedition.Freeze();

            r_SupplyOrDocking = new SolidColorBrush(Color.FromRgb(0xDE, 0xC7, 0x72));
            r_SupplyOrDocking.Freeze();

            r_Arsenal = new SolidColorBrush(Color.FromRgb(0xBA, 0x8F, 0x79));
            r_Arsenal.Freeze();

            r_Modernization= new SolidColorBrush(Color.FromRgb(0xCA, 0xA6, 0xDD));
            r_Modernization.Freeze();
        }

        public object Convert(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            var rCategory = (QuestCategory)rpValue;
            switch (rCategory)
            {
                case QuestCategory.Composition:
                    return r_Composition;

                case QuestCategory.Sortie:
                case QuestCategory.Sortie2:
                    return r_Sortie;

                case QuestCategory.Practice:
                    return r_Practice;

                case QuestCategory.Expedition:
                    return r_Expedition;

                case QuestCategory.SupplyOrDocking:
                    return r_SupplyOrDocking;

                case QuestCategory.Arsenal:
                    return r_Arsenal;

                case QuestCategory.Modernization:
                    return r_Modernization;

                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object rpValue, Type rpTargetType, object rpParameter, CultureInfo rpCulture)
        {
            throw new NotImplementedException();
        }
    }
}
