namespace Shared
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class PaginatedResult<T> : Result<T>
    {
        private List<T> _data;

        public PaginatedResult() : base()
        {
            _data = new List<T>();
        }

        [JsonConstructor]
        public PaginatedResult(bool success, List<T> data, int totalCount, int currentPage, int pageSize)
            : base()
        {
            Success = success;
            _data = data ?? new List<T>();
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        [JsonPropertyOrder(-2)]
        public new List<T> Data
        {
            get => _data;
            set
            {
                _data = value;
                base.Data = default;
            }
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        [JsonIgnore]
        public bool HasPreviousPage => CurrentPage > 1;

        [JsonIgnore]
        public bool HasNextPage => CurrentPage < TotalPages;

        public static PaginatedResult<T> Create(List<T> data, int totalCount, int currentPage, int pageSize)
        {
            return new PaginatedResult<T>(true, data, totalCount, currentPage, pageSize);
        }
    }
}