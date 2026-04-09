namespace DenunciaDo.Application.DTOs
{
    public class PaginatedListDto<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;
        public List<T> Items { get; set; } = new List<T>();
    }
}
