using DigiKala.DataAccessLayer.Context;
using DigiKala.DataAccessLayer.Entities;
using Parbad.Gateway.Mellat;
using Parbad.GatewayBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiKala.Core.Classes
{
	public class GatewaysAccounts : IGatewayAccountSource<MellatGatewayAccount>
	{
		private readonly DatabaseContext _context;

		public GatewaysAccounts(DatabaseContext context)
		{
			_context = context;
		}
		public async Task AddAccountsAsync(IGatewayAccountCollection<MellatGatewayAccount> accounts)
		{
			Setting setting = _context.Settings.First();
			accounts.Add(new MellatGatewayAccount()
			{
				TerminalId = setting.MellatTerminalId,
				UserName = setting.MellatUserName,
				UserPassword = setting.MellatPassword
			});
		}
	}
}
