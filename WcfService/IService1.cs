using ApplicationService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        //Users methods

        [OperationContract]
        List<UserDTO>GetAllUsers();

        [OperationContract]
        string PostUser(UserDTO userDto);

        [OperationContract]
        string DeleteUser(int id);
        [OperationContract]
        string UpdateUser(UserDTO userDto);

        //Friends methods

        [OperationContract]
        List<FriendshipDTO> GetAllFriends();

        [OperationContract]
        string PostFriend(FriendshipDTO friendDto);

        [OperationContract]
        string UpdateFriend(FriendshipDTO friendDto);

        [OperationContract]
        string DeleteFriend(int id);

        //Events methods

        [OperationContract]
        List<EventDTO> GetAllEvents();

        [OperationContract]
        string PostEvent(EventDTO friendDto);
        [OperationContract]
        string UpdateEvent(EventDTO eventDto);

        [OperationContract]
        string DeleteEvent(int id);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfService.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
