using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Combat;
using Godot;

public static class SkillHandler {
    public static readonly Skill WALK = Skill.Load("walk");
    public static readonly Skill[] SKILLS = {
        Skill.Load("burst"),
        Skill.Load("heal_wave"),
        Skill.Load("heal_word"),
        Skill.Load("snipe"),
        Skill.Load("rain_of_pain"),
        Skill.Load("shot"),
    };

    public static Skill FindRandomSkillFor(Entity entity) {
        return RandomSkillOut(entity.coreSkills.Where(s => s != null).Select(s => s.element));
    }

    public static void FillSkills(Entity entity) {
        entity.skill1 = FindRandomSkillFor(entity);
        entity.skill2 = FindRandomSkillFor(entity);
        entity.skill3 = FindRandomSkillFor(entity);
    }

    private static readonly string[][] PREFIXES = {
        new string[] {
            "Neutral",
        },
        new string[] {
            "Fire", "Fiery", "Heat"
        },
        new string[] {
            "Light", "Electric", "Thunder"
        },
        new string[] {
            "Metal", "Earth", "Metallic"
        },
        new string[] {
            "Wind", "Air"
        },
        new string[] {
            "Water", "Cold", "Ice", "Frost"
        },
        new string[] {
            "Nature", "Plant"
        },
        new string[] {
            "Dark", "Sinister"
        }
    };

    private static string ElementPrefix(Element element) {
        return PREFIXES[(int) (element)].Random();
    }

    public static Skill RandomTemplateOut(Element e, string[] tags) {
        List<Skill> skills = SKILLS.ToList();
        while (true) {
            Skill template = skills.Random();
            if (CheckTemplateOut(template, e, tags)) {
                return template;
            }
            skills.Remove(template);
        }
    }

    public static bool CheckTemplateOut(Skill template, Element e, string[] tags) {
        // Check element
        if (template.template != "*"
            && template.template.Contains(e.Letter())
            && (template.element != e || template.template != "")) {
            return false;
        }
        // Check tags
        foreach (string tag in tags) {
            if (template.tags.Contains(tag)) {
                return false;
            }
        }
        return true;
    }

    public static Skill RandomSkillIn(IEnumerable<Element> elements) {
        Skill template = (Skill) SKILLS.Random().Duplicate();
        Element element = elements.Random();
        template.element = element;
        template.name = ElementPrefix(element) + " " + template.name;
        return template;
    }

    public static Skill RandomSkillOut(IEnumerable<Element> elements) {
        List<Element> includedElements = ElementUtils.GetAllElements();
        includedElements.RemoveAll(e => elements.Contains(e));
        return RandomSkillIn(includedElements);
    }
}