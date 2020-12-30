using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using System.Text.RegularExpressions;

namespace ProjectRimFactory.Common
{

    public class PRF_FormatString_forXML : GameComponent
    {
        //Dict holds Keys & Values for the Translate
        private static Dictionary<string, string> valuePairs = new Dictionary<string, string>();

        public static void RegisterData(Dictionary<string, string> FormatKeys)
        {


            foreach (KeyValuePair<string, string> entry in FormatKeys)
            {
                if (entry.Key == null || valuePairs.ContainsKey(entry.Key))
                {
                    //Log.Error("PRF PRF_FormatString_forXML - Attempted to register a duplicate key");
                }
                else
                {
                    valuePairs[entry.Key] = entry.Value;
                }
            }
        }

        private static string GetData(string key)
        {
            return valuePairs?[key] ?? "";
        }

        private static String GetData(Match match)
        {
            return GetData(match.Groups["FormatKey"].Value);
        }


        public static string ParseFormat(string input)
        {
            
            Regex rx = new Regex(@"\|(?<FormatKey>.+?)\|",
          RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return rx.Replace(input, GetData); 
        }
    }







    [StaticConstructorOnStartup]
    public class CompPRFHelp : ThingComp
    {

        public static readonly Texture2D LaunchReportTex = ContentFinder<Texture2D>.Get("UI/Commands/LaunchReport", true);
        public string HelpText
        {
            get
            {
                if (Translator.TryTranslate($"{parent.def.defName}_HelpText", out TaggedString text))
                {
                    return text;
                }
                return null;
            }
        }
        public string OrdoText
        {
            get
            {
                if (Translator.TryTranslate($"{parent.def.defName}_OrdoText", out TaggedString text))
                {
                    return text;
                }
                return null;
            }
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra()) yield return g;
            string helpText = PRF_FormatString_forXML.ParseFormat(HelpText);

           

            if (!string.IsNullOrEmpty(helpText))
            {
                

                yield return new Command_Action
                {
                    defaultLabel = "PRFHelp".Translate(),
                    defaultDesc = "PRFHelpDesc".Translate(),
                    icon = LaunchReportTex,
                    action = () =>
                    {
                        if (Find.WindowStack.WindowOfType<Dialog_MessageBox>() == null)
                        {
                            Find.WindowStack.Add(new Dialog_MessageBox(helpText));
                        }
                    }
                };
            }
            if (PRFDefOf.PRFOrdoDataRummaging?.IsFinished == true) // == comparison between bool? and bool
            {
                string ordoText = OrdoText;
                if (!string.IsNullOrEmpty(ordoText))
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "PRFViewOrdo".Translate(parent.LabelCapNoCount),
                        icon = LaunchReportTex,
                        action = () =>
                        {
                            if (Find.WindowStack.WindowOfType<Dialog_MessageBox>() == null)
                            {
                                Find.WindowStack.Add(new Dialog_MessageBox(ordoText));
                            }
                        }
                    };
                }
            }
        }
    }
}
