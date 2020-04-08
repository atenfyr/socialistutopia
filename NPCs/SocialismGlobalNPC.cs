using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace SocialistUtopia.NPCs
{
    public class SocialismGlobalNPC : GlobalNPC
    {
        public override bool Autoload(ref string name)
        {
            // Make sure we can always buy everything
            IL.Terraria.Player.BuyItem += HookForceOne;
            IL.Terraria.Player.BuyItemOld += HookForceOne;
            IL.Terraria.Player.CanBuyItem += HookForceOne;

            // Make sure we can always sell items (contributing to society!)
            IL.Terraria.Player.SellItem += HookForceOne;

            // Cleaning up mess left over by no money
            IL.Terraria.Item.GetStoreValue += HookForceZero;
            IL.Terraria.Player.CollectTaxes += HookForceExit;
            if (ModLoader.GetMod("MedicareForAll") == null) IL.Terraria.Main.GUIChatDrawInner += HookFreeHealthcare;

            return base.Autoload(ref name);
        }

        private static void HookForceOne(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);
            c.Emit(Ldc_I4_1);
            c.Emit(Ret);
        }

        private static void HookForceZero(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);
            c.Emit(Ldc_I4_0);
            c.Emit(Ret);
        }

        private static void HookForceExit(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);
            c.Emit(Ret);
        }

        private void HookFreeHealthcare(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);

            // First we jump to the first nurse section (To change price label to say "Free")
            if (!c.TryGotoNext(i => i.MatchCall(typeof(PlayerHooks).GetMethod(nameof(PlayerHooks.ModifyNursePrice))))) return;
            c.Index++;
            var label = il.DefineLabel();
            c.Emit(Br, label);
            if (!c.TryGotoNext(i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons))))) return;
            c.Index -= 3;
            c.EmitDelegate<Func<string>>(() =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                return Lang.inter[54].Value + " (Free?)";
#pragma warning restore CS0618 // Type or member is obsolete
            });
            c.Index -= 3;
            c.MarkLabel(label);

            // Then we jump to the second nurse section (To skip past the check that it's not equal to zero)
            if (!c.TryGotoNext(i => i.MatchCall(typeof(Terraria.GameContent.Achievements.AchievementsHelper).GetMethod(nameof(Terraria.GameContent.Achievements.AchievementsHelper.HandleNurseService))))) return;
            if (!c.TryGotoPrev(i => i.Match(Ble))) return;

            // Place branch instruction before if statement
            var label2 = il.DefineLabel();
            c.Index -= 2;
            c.Emit(Br, label2);

            // Place branch endpoint after if statement
            c.Index += 3;
            c.MarkLabel(label2);
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == NPCID.TaxCollector)
            {
                Main.LocalPlayer.taxMoney = 0;
                chat = "The fact that I haven't been executed yet is shocking.";
            }
        }
    }
}
