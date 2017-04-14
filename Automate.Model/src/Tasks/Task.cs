using System;
using System.Text;

namespace Automate.Model.Tasks
{
    public abstract class Task
    {
        public Guid Guid { get; } = Guid.NewGuid();
        private Guid _assignedToGuid;
        public bool IsAssigned { get; private set; }

        public Guid AssignedToGuid
        {
            get
            {
                if (!IsAssigned)
                    throw new Exception();
                return _assignedToGuid;
            }
            set
            {
                IsAssigned = true;
                _assignedToGuid = value;
            }
        }
    }
}