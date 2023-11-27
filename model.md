@startuml

class Address{
    +Street : string
    +StreetNr : string
    +City : string
    +State : string
    +Zip : int 
}

class Clubs {
   +Id : int
   +ClubName : string
   +BillingAddress : Address
   +Players: List<Player>
   +Currentleague : int
}

class Players{
   +Id : int
   +Firstname: string
   +Lastname: string
   +GebDat: DateTime
   +Gender: int
   +Marketvalue: decimal
   +Address: Address
}


Clubs "1" o-- "n" Players
Clubs "1" -- "1" Address 
Players "1" -- "1" Address



@enduml