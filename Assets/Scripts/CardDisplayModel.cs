using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RealtimeModel]
public partial class CardDisplayModel
{
    [RealtimeProperty(1, true, true)] private string _sprite;
}