using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;

namespace BokehLab.InteractiveDof
{
    class AbstractRendererModule : IRendererModule
    {
        protected int Width { get; set; }
        protected int Height { get; set; }

        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (enabled)
                    {
                        Enable();
                    }
                    else
                    {
                        Disable();
                    }
                }
            }
        }

        public AbstractRendererModule()
        {
            enabled = false;
        }

        #region IRendererModule Members

        public virtual void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public virtual void Dispose()
        {
            if (Enabled)
            {
                Disable();
            }
        }

        protected virtual void Enable() { }

        protected virtual void Disable() { }

        public virtual void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            if (Enabled)
            {
                Disable();
                Enable();
            }
        }

        public virtual void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
        }

        #endregion
    }
}
