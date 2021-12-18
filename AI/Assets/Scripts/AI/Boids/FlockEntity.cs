using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockEntity : MonoBehaviour
{
    public float speed = 1;

    [HideInInspector]
    public GlobalFlock flock;
    
    float rotSpeed = 10;
    float neighbourDistance = 3f;
    bool turning = true;

    

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(0.5f, 2f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < flock.xboundMin || transform.position.x > flock.xboundMax)
            turning = true;
        else if (transform.position.y < flock.yboundMin || transform.position.y > flock.yboundMax)
            turning = true;
        else if (transform.position.z < flock.zboundMin || transform.position.z > flock.zboundMax)
            turning = true;
        else turning = false;

        if (turning)
        {
            Vector3 direction = flock.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(direction),
                                                  rotSpeed * Time.deltaTime);
            speed = Random.Range(0.5f, 3f);
        }
        //20% of frames do steering forces on each flock entity
        if (Random.Range(0,5) < 1)
        {
            //good stuff
            GameObject[] goFish;
            goFish = flock.fish;

            Vector3 vcentre = this.transform.position; //cohesion
            Vector3 vavoid = new Vector3(0, 0, 0); //seperation

            float gSpeed = 0.5f;
            Vector3 goalPos = flock.goalPos;

            float dist;
            int groupSize = 0;

            foreach(GameObject go in goFish)
            {
                if(go != this.gameObject)
                {
                    dist = Vector3.Distance(go.transform.position, this.transform.position);
                    if (dist <= neighbourDistance)
                    {
                        //cohesion
                        vcentre += go.transform.position;
                        groupSize++;

                        //seperation
                        if (dist < 6f)
                        {
                            vavoid = vavoid + (this.transform.position - go.transform.position);
                        }
                        FlockEntity anotherFlockEntity = go.GetComponent<FlockEntity>();
                        gSpeed += anotherFlockEntity.speed;
                    }
                }
            }
            if(groupSize >= 0)
            {
                vcentre = vcentre / groupSize + (goalPos - this.transform.position); // cohesion
                speed = (gSpeed / groupSize) + Random.RandomRange(-0.1f, 0.1f);
                if(speed > 5 || speed < 0)
                {
                    speed = Random.Range(.5f, 3f);
                }
                Vector3 direction = (vcentre + vavoid) - transform.position; // alignment
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      rotSpeed * Time.deltaTime);
            }

        }

        transform.Translate(0, 0, Time.deltaTime * speed);
    }
}
