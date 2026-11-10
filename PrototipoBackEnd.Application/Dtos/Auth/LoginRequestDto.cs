namespace PrototipoBackEnd.Application.Dtos.Auth
{
	public class LoginRequestDto
	{
		[System.ComponentModel.DataAnnotations.Required]
		public string Email { get; set; } = string.Empty;
		[System.ComponentModel.DataAnnotations.Required]
		public string Senha { get; set; } = string.Empty;
	}
}
