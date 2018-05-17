namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Parsers
{
    delegate Result<T> Parser<T>(string rpInput);
}
