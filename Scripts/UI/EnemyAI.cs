using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async void Play() {
        await Task.Delay(1000);
        var reachableTiles = BoardUtils.AllReachableTiles(piece.on, maxLength: 3, valid: CanStepOn);
        // TODO: Think
        var pair = reachableTiles.ToList().Random();
        if (pair.Key != piece.on) {
            SkillArea a = new SkillArea();
            a.AddRange(pair.Value);
            await SkillHandler.WALK.Apply(piece, a);
        }

        var skills = actor.GetCoreSkills();
        float bestHeuristic = 0;
        Skill bestSkill = null;
        SkillArea bestArea = null;
        foreach (Skill skill in skills) {
            if (!skill.condition.IsValid()) {
                continue;
            }
            // Try Cone
            skill.area.launcher = piece;
            if (skill.area is Cone cone) {
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
            else if (skill.area is Target target) {
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
            await bestSkill.Apply(piece, bestArea);
        }
        Global.battle.NextTurn();
    }
}