using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.SkillAreas;
using Godot;

public class EnemyAI {
    private Piece piece;
    private Entity actor;

    public EnemyAI(Piece piece) {
        this.piece = piece;
        actor = piece.entity;
    }

    public bool CanStepOn(Tile tile) {
        return tile.pieces.Count == 0;
    }

    public void Play() {
        var reachableTiles = BoardUtils.AllReachableTiles(piece.on, valid: CanStepOn);
        // TODO: Think
        var pair = reachableTiles.ToList().Random();
        SkillArea a = new SkillArea();
        a.AddRange(pair.Value);
        SkillHandler.WALK.effect.Apply(Element.NONE, piece, a);

        Skill[] skills = actor.coreSkills;
        float bestHeuristic = 0;
        Skill bestSkill = null;
        SkillArea bestArea = null;
        foreach (Skill skill in skills) {
            // Try Cone
            skill.area.launcher = piece;
            Cone cone = skill.area as Cone;
            if (cone != null) {
                foreach (Direction dir in DirectionUtils.DIRECTIONS) {
                    SkillArea area = cone.SkillAreaIfTarget(dir);
                    float heuristic = skill.effect.Heuristic(skill.element, piece, area);
                    if (heuristic > bestHeuristic) {
                        bestHeuristic = heuristic;
                        bestArea = area;
                        bestSkill = skill;
                    }
                }
            }
            // Try Target
            Target target = skill.area as Target;
            if (target != null) {
                foreach (Tile tile in BoardUtils.AllTiles(t => target.CanSelect(t))) {
                    SkillArea area = target.SkillAreaIfTarget(tile);
                    float heuristic = skill.effect.Heuristic(skill.element, piece, area);
                    if (heuristic > bestHeuristic) {
                        bestHeuristic = heuristic;
                        bestArea = area;
                        bestSkill = skill;
                    }
                }
            }
        }
        if (bestSkill != null) {
            bestSkill.effect.Apply(bestSkill.element, piece, bestArea);
        }
        Global.battle.NextTurn();
    }
}