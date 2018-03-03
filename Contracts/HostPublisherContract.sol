pragma solidity ^0.4.4;

contract HostPublisherContract{
    string[] public hosts;
    uint cont = 0;
    
    function AddHost(string newHost) 
    {
        hosts.push(newHost);
    }
}