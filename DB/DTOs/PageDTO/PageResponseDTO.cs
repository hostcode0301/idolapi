namespace idolapi.DB.DTOs
{
    public class PageResponseDTO<T>
    {
        public int TotalRecords { get; set; } = 1;
        public T Payload { get; set; }

        public PageResponseDTO() { }

        public PageResponseDTO(int totalRecord, T payload)
        {
            this.TotalRecords = totalRecord;
            this.Payload = payload;
        }
    }
}