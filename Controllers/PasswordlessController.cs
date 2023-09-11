using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passwordless.Net;
using PasswordlessDemo.Models;

namespace PasswordlessDemo.Controllers
{
	/// <summary>
	///     Sign in - Verify the sign in
	///     The passwordless API handles all the cryptography and WebAuthn details so that you don't need to.
	///     In order for you to verify that the sign in was successful and retrieve details such as the username, you need to
	///     verify the token that the passwordless client side code returned to you.
	///     This is as easy as POST'ing it to together with your ApiSecret.
	///     Please see: https://docs.passwordless.dev/guide/api.html#signin-verify
	/// </summary>
	/// <param name="token"></param>
	/// <returns></returns>
	
	[Route("api/[controller]")]
	[ApiController]
	public class PasswordlessController : ControllerBase
	{
		private readonly IPasswordlessClient _passwordlessClient;
		public PasswordlessController(IPasswordlessClient passwordlessClient)
		{
			_passwordlessClient = passwordlessClient;
		}

		[HttpPost]
		[Route("signup")]
		public async Task<IActionResult> GetRegisterToken(UserModel alias)
		{
			var guid = Guid.NewGuid().ToString();
			
			var payload = new RegisterOptions(guid, alias.Username)
			{
				Aliases = new HashSet<string> { alias.Username }
			};
			try
			{
				var token = await _passwordlessClient.CreateRegisterTokenAsync(payload);
				return Ok(token);
			}
			catch (PasswordlessApiException e)
			{
				return new JsonResult(e.Details)
				{
					StatusCode = (int?)e.StatusCode,
				};
			}

		}


		[HttpGet]
		[Route("VerifySignin")]
		public async Task<IActionResult> VerifySignInToken(string token)
		{
			try
			{
				var verifiedUser = await _passwordlessClient.VerifyTokenAsync(token);
				return Ok(verifiedUser);
			}
			catch (PasswordlessApiException e)
			{
				return new JsonResult(e.Details)
				{
					StatusCode = (int)e.StatusCode
				};
			}
		}

	}
}
