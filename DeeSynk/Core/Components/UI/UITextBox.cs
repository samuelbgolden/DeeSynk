﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Components.Types.UI;
using OpenTK;

namespace DeeSynk.Core.Components.UI
{
    public class UITextBox : UIElement
    {
        public override string ID_GLOBAL_DEFAULT => "TEXT_BOX";

        public override void ClickAt(float time, MousePosition mouseClick, MouseMove mouseMove)
        {
            throw new NotImplementedException();
        }

        public override bool Update(float time)
        {
            throw new NotImplementedException();
        }
    }
}
