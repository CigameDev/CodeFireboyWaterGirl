using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [RequireComponent(typeof(LineRenderer))]
    public class Laser : MonoBehaviour
    {
        [SerializeField]private LaserColor _lineColor;

        const int Infinity = 999;

        int maxReflections = 100;
        int currentReflections = 0;

        Vector2 startPoint, direction;
        List<Vector3> Points;
        int defaultRayDistance = 100;
        LineRenderer lr;

        private List<Collider2D> _hits2D;
        private List<Collider2D> _tempLaser;

        // Start is called before the first frame update
        void Start()
        {
            Points = new List<Vector3>();
            _hits2D = new List<Collider2D>();
            _tempLaser = new List<Collider2D>();

            lr = transform.GetComponent<LineRenderer>();
            startPoint = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            direction = this.transform.right;
            int layerMask = 1 << 10;
            var hitData = Physics2D.Raycast(startPoint, direction.normalized, defaultRayDistance,~layerMask);

            currentReflections = 0;
            Points.Clear();
            Points.Add(startPoint);
            _hits2D.Clear();

            if (hitData)
            {
                _hits2D.Add(hitData.collider);
                ReflectFurther(startPoint, hitData);
                
            }
            else
            {
                Points.Add(startPoint + direction.normalized * Infinity);
            }

            lr.positionCount = Points.Count;
            lr.SetPositions(Points.ToArray());

            List<Collider2D> lasers = this.GetLaser();
            if (lasers.Count > 0)
            {
                for(int i = 0; i < lasers.Count; i++)
                {
                    if (!_tempLaser.Contains(lasers[i]))
                    {
                        _tempLaser.Add(lasers[i]);
                        lasers[i].GetComponent<LaserEvent>()?.LaserOpen();
                    }
                }
            }
            else
            {
                if(_tempLaser.Count > 0)
                {
                    for(int i = 0; i < _tempLaser.Count; i++)
                    {
                        _tempLaser[i].GetComponent<LaserEvent>()?.LaserClose();
                    }

                    _tempLaser.Clear();
                }
            }

        }

        private List<Collider2D> GetLaser()
        {
            List<Collider2D> lasers = new List<Collider2D>();
            for (int i = 0; i < _hits2D.Count; i++)
            {
                if (_hits2D[i].tag == TagDefine.LASER)
                {
                    LaserEvent evt = _hits2D[i].GetComponent<LaserEvent>();
                    if(evt.Color() == _lineColor)
                        lasers.Add(_hits2D[i]);
                }
                    
            }

            return lasers;
        }

        private void ReflectFurther(Vector2 origin, RaycastHit2D hitData)
        {
            if (currentReflections > maxReflections) return;
            
            
            Points.Add(hitData.point);
            currentReflections++;

            Vector2 inDirection = (hitData.point - origin).normalized;
            Vector2 newDirection = Vector2.Reflect(inDirection, hitData.normal);

            if (hitData.collider.tag != "Respawn") return;

            int layerMask = 1 << 10;
            var newHitData = Physics2D.Raycast(hitData.point + (newDirection * 0.0001f), newDirection * 100, defaultRayDistance,~layerMask);
            if (newHitData)
            {
                _hits2D.Add(newHitData.collider);
                ReflectFurther(hitData.point, newHitData);
            }
            else
            {
                Points.Add(hitData.point + newDirection * defaultRayDistance);
            }
        }
    }
}