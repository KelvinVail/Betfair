namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    /// <summary>
    /// The request base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the parameter object.
    /// </typeparam>
    internal class RequestBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBase{T}"/> class.
        /// </summary>
        /// <param name="params">
        /// The params.
        /// </param>
        internal RequestBase(T @params)
        {
            this.Params = @params;
            const string Name = nameof(T);
            this.Method = "SportsAPING/v1.0/" + char.ToLowerInvariant(Name[0]) + Name.Substring(1);
        }

        /// <summary>
        /// The ID.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        internal int Id => 1;

        /// <summary>
        /// The json RPC.
        /// </summary>
        [JsonProperty(PropertyName = "jsonrpc")]
        internal string Jsonrpc => "2.0";

        /// <summary>
        /// Gets the method.
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        internal string Method { get; }

        /// <summary>
        /// Gets the params.
        /// </summary>
        [JsonProperty(PropertyName = "params")]
        internal T Params { get; }
    }
}