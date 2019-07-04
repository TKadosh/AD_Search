using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace AD_search
{

    class Program
    {
        static void Main(string[] args)
        {
            //Main
            //get string from user
            Console.WriteLine("search : ");
            string userInput = Convert.ToString(Console.ReadLine());
            //domain param
            string domainName = "your@domain";
            printList(search_by(userInput, domainName));
            Console.ReadKey();
        }
        //return list of users contains userInput == phone number || name || email
        public static List<DirectoryEntry> search_by(string userInput, string domainName)
        {
            //create principal obj
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, domainName);
            UserPrincipal grp = new UserPrincipal(ctx);
            PrincipalSearcher srch = new PrincipalSearcher(grp);
            List<DirectoryEntry> userLst = new List<DirectoryEntry>();

            //check userPrincipal is not empty
            if (grp != null)
            {
                //search string in AD
                foreach (Principal found in srch.FindAll())
                {
                    //create obj
                    DirectoryEntry de = found.GetUnderlyingObject() as DirectoryEntry;
                    //has value?
                    if (IsActive(de) && found is UserPrincipal && de.Properties["telephoneNumber"].Value != null && de.Properties["mail"].Value != null)
                    {
                        //contains userInput?
                        if (de.Properties["telephoneNumber"].Value.ToString().Contains(userInput) || de.Properties["mail"].Value.ToString().Contains(userInput) || found.DisplayName.ToString().ToLower().Contains(userInput.ToLower()))
                        {

                            //Console.WriteLine(found.SamAccountName + " - " + found.DisplayName + " - " + de.Properties["mail"].Value + " - " + de.Properties["telephoneNumber"].Value);
                            userLst.Add(de);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("\nWe did not find that user in that domain, perhaps the group resides in a different domain?");
            }
            grp.Dispose();
            ctx.Dispose();
            return userLst;
        }
        //IsActive user
        public static bool IsActive(DirectoryEntry de)
        {
            if (de.NativeGuid == null) return false;
            int flags = (int)de.Properties["userAccountControl"].Value;
            return !Convert.ToBoolean(flags & 0x0002);
        }

        //print method
        public static void printList (List<DirectoryEntry> userLst)
        {
            if (userLst != null) {
                foreach (DirectoryEntry de in userLst)
                {
                    Console.WriteLine("Name - " + de.Properties["DisplayName"].Value + " Mail - " + de.Properties["mail"].Value + " Main Phone - " + de.Properties["telephoneNumber"].Value + " Extention - " + de.Properties["ipPhone"].Value + " Mobile - " + de.Properties["mobile"].Value);

                }
            }
            else
            {
                Console.WriteLine("list is empty");
            }
        }
    }
}

