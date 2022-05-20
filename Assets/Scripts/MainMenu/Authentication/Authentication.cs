using System;
using UnityEngine;

namespace MainMenu.Authentication
{
    [Serializable]
    public static class Authentication 
    {
        // public AuthenticationUI UI;

        private static bool isAuthenticated = false;

        public static bool IsAuthenticated
        {
            get
            {
                Debug.Log(isAuthenticated ? "Authentication Successfull" : "Authentication Failed");

                return isAuthenticated;
            }
            set
            {
                isAuthenticated = value;
            }
        }


//could use a event for auth

//do login auth input -> verify

        // public static bool CheckAuthentication()
        // {
        //     if (isAuthenticated)
        //     {
        //         return true;
        //     }
        //
        //     
        //     return false;
        // }
    
    
        public static void Authenticate()
        {
            isAuthenticated = true;
            // CheckAuthentication();
        }
    




    }
}