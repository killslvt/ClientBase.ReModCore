using ClientBase.Features.Movement;
using ClientBase.SDK;
using ReMod.Core.UI.QuickMenu;
using VRC.SDKBase;

namespace ClientBase.Loader.Menus
{
    internal class MovementMenu : Module
    {
        public override void OnUiManagerInit()
        {
            ReCategoryPage movementMenu = QMLoader.uiManager.QMMenu.AddCategoryPage("Movement", "Movement Features", null, "#FFFFFF");
            ReMenuCategory catagory = movementMenu.AddCategory("Movement");

            catagory.AddButton("Force Jump", "", delegate
            {
                Networking.LocalPlayer.SetJumpImpulse(1f);
                PopupUtils.HudMessage("Movement", "Forced Jump Enabled", 3);
            }, null, "#FFFFFF");

            catagory.AddToggle("Flight", "Allows You To Fly", (Enabled) =>
            {
                Flight.FlyEnabled = Enabled;

                if (Flight.FlyEnabled)
                {
                    Flight.FlyEnabled = true;
                    PopupUtils.HudMessage("Movement", "Flight Toggled On", 3);
                }
                else
                {
                    Flight.FlyEnabled = false;
                    PopupUtils.HudMessage("Movement", "Flight Toggled Off", 3);
                }
            }, "#FFFFFF");

            catagory.AddToggle("Speed Hack", "Allows You To Move Fast", (Enabled) =>
            {
                SpeedHack.IsEnabled = Enabled;

                if (SpeedHack.IsEnabled)
                {
                    SpeedHack.Enable();
                    PopupUtils.HudMessage("Movement", "Speed Hack Toggled On", 3);
                }
                else
                {
                    SpeedHack.Disable();
                    PopupUtils.HudMessage("Movement", "Speed Hack Toggled Off", 3);
                }
            }, "#FFFFFF");
        }
    }
}
