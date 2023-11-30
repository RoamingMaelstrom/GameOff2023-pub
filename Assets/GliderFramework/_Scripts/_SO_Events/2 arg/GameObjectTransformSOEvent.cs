using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOEvents
{
[CreateAssetMenu(fileName = "GameObjectTransformSOEvent", menuName = "SOEvent/2arg/GameObjectTransformSOEvent", order = 230)]
public class GameObjectTransformSOEvent : SOEvent<GameObject, Transform>{}
}

