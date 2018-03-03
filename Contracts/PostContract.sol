pragma solidity ^0.4.4;

contract PostContract{
    string public Url;
    string public HashContent;
    
    function AddPostContract(string url, string hashContent) 
    {
        Url = url;
        HashContent = hashContent;
    }
}