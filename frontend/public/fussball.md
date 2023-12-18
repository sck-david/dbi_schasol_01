# Class Diagram

```plantuml
@startuml
class Car {
  - make: String
  - model: String
  + start()
  + drive()
  + stop()
}

class Engine {
  - horsepower: int
  + start()
  + stop()
}

Car *- Engine : has

@enduml
