using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModularMod.Past.Prefabs.Objects
{
    public class InitObjects
    {
        public static void Init()
        {
            PastEntranceControllerObject.Init();
            WarningSticker.Init();
            WoodenCrate.Init();
            ShippingContainer.Init();
            WarpGates.Init();
            ExpressElevator.Init();

            YellowLights.InitYellowLightHorizontal();
            YellowLights.InitYellowLightVertical();

            MetalFences.Init();

            RedLight.Init();
            SniperTurretsDefault.InitDownward();
            SniperTurretsDefault.InitLeft();
            SniperTurretsDefault.InitRight();

            SniperTurretsProfessional.InitDownward();
            SniperTurretsProfessional.InitLeft();
            SniperTurretsProfessional.InitRight();

            MovingTile.Init();
            ForbodingSign.Init();

            Sprite_Decals.Init();
            FakeCorridor.Init();
            BigVaultObject.Init();

            Engine.Init();
            BigFuckOffDoor.Init();
            TriggerEncounter.Init();
            BigSwitch.Init();
            BigFuckOffDoorAuto.Init();

            SteelPanopticonTrigger.Init();

            Spaceship.Init();
            SpaceShiptrigger.Init();
            ZappyDoor.Init();
            BloodRedLightness.Init();
            ArmorCrate.Init();

            Dead.Init();
        }
    }
}
