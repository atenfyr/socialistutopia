using Terraria.ModLoader;

namespace SocialistUtopia
{
	public class SocialistUtopia : Mod
	{
        public static string GithubUserName => "atenfyr";
        public static string GithubProjectName => "socialistutopia";

        public SocialistUtopia()
		{
            Properties = ModProperties.AutoLoadAll;
        }
	}
}