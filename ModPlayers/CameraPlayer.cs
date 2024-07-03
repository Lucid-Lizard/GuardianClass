using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace GuardianClass.ModPlayers
{
    public class CameraPlayer : ModPlayer
    {
        public class CameraShake
        {
            public float duration;
            public float intensity;
            public float maxDuration;
            public CameraShake(float duration, float intensity)
            {
                this.duration = duration;
                this.intensity = intensity;
                maxDuration = duration;
            }

            

            public float GetIntensity()
            {
                return MathHelper.Lerp(0f, intensity, duration / maxDuration);
            }
            public void Kill()
            {
                Main.player[Main.myPlayer].GetModPlayer<CameraPlayer>().cameraShakes.Remove(this);                
            }
        }

        public List<CameraShake> cameraShakes = new List<CameraShake>();

        public void AddCameraShake(float duration, float intensity)
        {
            cameraShakes.Add(new CameraShake(duration, intensity));
        }

        public override void PostUpdate()
        {
            for(int i = 0; i < cameraShakes.Count; i++)
            {
                CameraShake c = cameraShakes[i];
                c.duration -= 1;
                //Main.NewText(c.duration);
                if (c.duration <= 0)
                {
                    c.Kill();
                }
            }
        }

        public override void ModifyScreenPosition()
        {
            for (int i = 0; i < cameraShakes.Count; i++)
            {
                Main.screenPosition += new Microsoft.Xna.Framework.Vector2(Main.rand.Next(-1,2), Main.rand.Next(-1, 2)) * cameraShakes[i].GetIntensity();
            }
        }
    }
}
