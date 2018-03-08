﻿using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectRimFactory.Common;
using UnityEngine;

namespace ProjectRimFactory.Industry
{
    [StaticConstructorOnStartup]
    public class CompPaperclipPowerPlant : CompPowerPlant
    {
        static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel", true);

        public int fuelPerSecond = 1;
        public int currentPowerModifierPct = 100;
        int maxPowerModifierPct = 100;

        protected float PowerProductionModifier
        {
            get
            {
                return (currentPowerModifierPct * fuelPerSecond) / 100f;
            }
        }

        protected override float DesiredPowerOutput
        {
            get
            {
                return -Props.basePowerConsumption * PowerProductionModifier;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.IsHashIntervalTick(60))
            {
                if (PRFDefOf.PaperclipGeneratorQuantumFoamManipulation.IsFinished)
                {
                    maxPowerModifierPct = 10000;
                }
                else if (PRFDefOf.PaperclipGeneratorKugelblitz.IsFinished)
                {
                    maxPowerModifierPct = 2500;
                }
                else if (PRFDefOf.PaperclipGeneratorSelfImprovement.IsFinished)
                {
                    maxPowerModifierPct = 500;
                }
                else
                {
                    maxPowerModifierPct = 100;
                }

                if (currentPowerModifierPct < maxPowerModifierPct)
                {
                    currentPowerModifierPct++;
                }

                parent.GetComp<CompRefuelable>().ConsumeFuel(fuelPerSecond);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra()) yield return g;
            yield return new Command_Action()
            {
                defaultLabel = "SetPaperclipConsumptionPerSecond".Translate(),
                defaultDesc = "SetPaperclipConsumptionPerSecond_Desc".Translate(),
                icon = SetTargetFuelLevelCommand,
                action = () => Find.WindowStack.Add(new Dialog_Slider(j => "PaperclipFuelConsumption".Translate(j), 1, 100, i => fuelPerSecond = i, fuelPerSecond))
            };
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.CompInspectStringExtra());
            builder.Append("PaperclipGeneratorEfficiency".Translate(currentPowerModifierPct, maxPowerModifierPct));
            return builder.ToString();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref fuelPerSecond, "fuelPerSecond", 1);
            Scribe_Values.Look(ref currentPowerModifierPct, "currentPowerModifier", 100);
        }
    }
}
