using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteRPG.Data
{
    public class CameraMovementState
    {
        CameraMovement current = 0;

        public void SetMoving(CameraMovement movement)
        {
            current |= movement;
        }

        public void ClearMoving(CameraMovement movement)
        {
            current &= ~movement;
        }

        public bool IsSet(CameraMovement movement)
        {
            return (current & movement) != 0;
        }

        public CameraMovement CurrentMovement { get { return current; } }

        public void Reset()
        {
            current = 0;
        }
    }
}
