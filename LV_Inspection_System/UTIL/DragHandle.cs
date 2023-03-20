﻿using System.Drawing;

namespace LV_Inspection_System.UTIL
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2014 Cyotek.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See imagebox-license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  public class DragHandle
  {
    #region Public Constructors

    public DragHandle(DragHandleAnchor anchor)
      : this()
    {
      this.Anchor = anchor;
    }

    #endregion

    #region Protected Constructors

    protected DragHandle()
    {
      this.Enabled = true;
      this.Visible = true;
    }

    #endregion

    #region Public Properties

    public DragHandleAnchor Anchor { get; protected set; }

    public Rectangle Bounds { get; set; }

    public bool Enabled { get; set; }

    public bool Visible { get; set; }

    #endregion
  }
}
