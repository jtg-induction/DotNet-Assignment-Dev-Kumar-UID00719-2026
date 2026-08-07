namespace dotNetAssignment.Models.DTO
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> d636554 ([DS_DA_01] : Implemented authentication and testases)
