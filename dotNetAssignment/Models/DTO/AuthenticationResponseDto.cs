namespace dotNetAssignment.Models.DTO
{
    public class AuthenticationResponseDto
    {
        /// <summary>
        /// The access token issued upon successful authentication. 
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token issued upon successful authentication. 
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
