using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace shader_test
{
    public class PhysicsChain
    {
        public static readonly float GRAVITY = 80.0f;
        public static readonly float STRING_ELASTICITY = 50.0f;
        public static readonly float STRING_TAUT_PROPORTION = 0.2f;

        private Vector2 _anchor;
        private List<PhysicsChainLink> _links;

        private float _linkLen;

        public PhysicsChain(Vector2 anchor, int numLinks, float linkLen)
        {
            this._anchor = anchor;

            this._links = new List<PhysicsChainLink>();
            for (int i = 1; i <= numLinks; i++) {

                Vector2 linkPos = new Vector2(
                    anchor.X + ((float)i * linkLen),
                    anchor.Y
                );

                PhysicsChainLink newLink = new PhysicsChainLink(linkPos, Vector2.Zero);
                _links.Add(newLink);
            }

            this._linkLen = linkLen;
        }

        public List<Vector2> GetLinkPoses()
        {
            List<Vector2> res = new List<Vector2>();
            res.Add(_anchor);

            for (int i = 0; i < _links.Count; i++) {
                res.Add(_links[i].pos);
            }

            return res;
        }

        public void Update(float timeElapsed)
        {
            for (int i = 0; i < _links.Count; i++) {
                Vector2 newVel = _links[i].vel;

                // gravity
                newVel += new Vector2(0.0f, GRAVITY) * timeElapsed;

                // left link
                Vector2? leftPos = GetLeftPos(i);
                if (leftPos != null)
                {
                    Vector2 towardVec = leftPos.Value - _links[i].pos;
                    newVel = AddTensionToVelocity(newVel, towardVec, timeElapsed);
                }

                // right link
                Vector2? rightPos = GetRightPos(i);
                if (rightPos != null)
                {
                    Vector2 towardVec = rightPos.Value - _links[i].pos;
                    newVel = AddTensionToVelocity(newVel, towardVec, timeElapsed);
                }

                // applying new velocity
                _links[i].pos = _links[i].pos + newVel * timeElapsed;
                _links[i].vel = newVel;
            }
        }

        private Vector2 AddTensionToVelocity(Vector2 startVelocity, Vector2 towardVec, float timeElapsed)
        {
            float extraLen = towardVec.Length() - _linkLen;
            if (extraLen < 0.0f) {
                // floppy string
                return startVelocity;
            } else {
                float extraProportion = extraLen / _linkLen;
                if (extraProportion < STRING_TAUT_PROPORTION) {
                    // stretchy string
                    return startVelocity + Vector2.Normalize(towardVec) * extraLen * STRING_ELASTICITY * timeElapsed;
                } else {
                    // taut string
                    Vector2 projection = (Vector2.Dot(startVelocity, towardVec) / Vector2.Dot(towardVec, towardVec)) * towardVec;
                    Vector2 rejection = startVelocity - projection;
                    return rejection + Vector2.Normalize(towardVec) * _linkLen * STRING_TAUT_PROPORTION * STRING_ELASTICITY * timeElapsed;
                }
            }
        }

        private Vector2? GetLeftPos(int linkInd)
        {
            if (linkInd == 0)
                return _anchor;
            else
                return _links[linkInd - 1].pos;
        }

        private Vector2? GetRightPos(int linkInd)
        {
            if (linkInd == _links.Count - 1)
                return null;
            else
                return _links[linkInd + 1].pos;
        }
    }

    public class PhysicsChainLink
    {
        public Vector2 pos;
        public Vector2 vel;

        public PhysicsChainLink(Vector2 pos, Vector2 vel)
        {
            this.pos = pos;
            this.vel = vel;
        }
    }
}