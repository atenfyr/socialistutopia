using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SocialistUtopia.Items
{
    public class SocialismGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            item.value = 0;
            if ((item.type >= ItemID.CopperCoin && item.type <= ItemID.PlatinumCoin) || item.type == ItemID.DefenderMedal)
            {
                item.type = ItemID.None;
            }
        }
    }
}
