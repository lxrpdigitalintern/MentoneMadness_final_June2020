using UnityEngine;
using System.Collections;

namespace PathCreation
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class CarScript : MonoBehaviour
    {
        private PathFollower pathFollower;

        // Traffic Control Members
        public bool _isStopped = false;
        public bool _canCollide = true;
        bool _isFront = true;

        Rigidbody _temp = null;
        public TrafficLightScript _trafficCont = null;
        public CarScript _frontCar = null;

        private void Awake()
        {
            pathFollower = GetComponent<PathFollower>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isStopped && !_canCollide && other == this)
                return; 

            if (other.transform.tag == "TrafficLight")
            {
                Debug.Log(transform.name + " Colliding with Traffic Light");

                // Check is this is the frontmost car
                if(_frontCar != null && !_isFront)
                {
                    Debug.Log(transform.name + " _frontCar is not null or _isFront is false - OnTrigger TrafficLight************************");
                    return;
                }

                _isFront = true;
                _frontCar = this;

                // Add car to the traffic list
                _trafficCont = other.GetComponent<TrafficLightScript>();
                _trafficCont.AddToTrafficList(this);

                StopCar();  
            }

            if (other.transform.tag == "Vehicle")
            {
                Debug.Log(transform.name + " Colliding with " + other.transform.name);

                StopCar();

                // Get the collided car's script
                CarScript otherCar = other.GetComponent<CarScript>();
              
                // Get reference to the front car
                _frontCar = otherCar._frontCar;

                if (_frontCar == null)
                {
                    Debug.Log(transform.name + " : _frontCar of " + other.transform.name + "is null - OnTrigger Vehicle**************************");
                    return;
                }

                // Add car to the traffic list
                _trafficCont = otherCar._trafficCont;
                _trafficCont.AddToTrafficList(this);
            }
        }

        /*
        private void OnTriggerExit(Collider other)
        {
            if (!_isStopped && _canCollide)
                return;

            if (other.transform.tag == "Vehicle")
                ResumeCar();
        }
        */

        void StopCar()
        {
            Debug.Log("Stopping Car " + transform.name);

            if (_isStopped && _temp != null)
                return;

            // Add rigid body
            _temp = gameObject.AddComponent<Rigidbody>();
            _temp.isKinematic = true;
            _temp.useGravity = false;

            // Stop car
            _isStopped = true;
            pathFollower.speed = 0;
        }

        public void ResumeCar()
        {
            Debug.Log("******************Resuming Car " + transform.name);

            pathFollower.speed = 2.5f;
            _canCollide = false;

            StartCoroutine(CanCollideCooldown());
        }

        IEnumerator CanCollideCooldown()
        {
            yield return new WaitForSeconds(3.0f);
            _canCollide = true;

            //Reset all members
            Destroy(_temp);
            _frontCar = null;
            _trafficCont = null;

            _isStopped = false;
            _isFront = false;
        }
    }
}