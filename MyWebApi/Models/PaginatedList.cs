namespace MyWebApi.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPage {  get; set; }

        public PaginatedList(List<T> items, int totalPage, int pageIndex, int pageSize)
        { 
            PageIndex = pageIndex;
            TotalPage = totalPage;
            AddRange(items);
        }

        public static PaginatedList<T> Create(IQueryable<T> src, int pageIndex, int pageSize)
        {
            var count = src.Count();
            var totalPage = (int)Math.Ceiling((double)count / pageSize);

            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPage) pageIndex = totalPage;

            var items = src.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, totalPage, pageIndex, pageSize);
        }
    }
}
