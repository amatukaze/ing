namespace HeavenlyWind
{
    struct DependencyLoadingInfo
    {
        public string Name { get; }
        public StatusCode StatusCode { get; }

        public DependencyLoadingInfo(string name, StatusCode statusCode)
        {
            Name = name;
            StatusCode = statusCode;
        }
    }
}
