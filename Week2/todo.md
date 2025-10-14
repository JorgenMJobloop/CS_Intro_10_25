# TODO 13.10.2025
    ## 

# Application
    ## Library application
        - Help the library keep track of which books are rented out, and which ones are not.
 

# Classes, Interfaces & ENUM
    
    Classes (marked: normal, static, abstract, internal)

        Media classes:
            - Book -> normal
            - Dvd -> normal
            - Vhs -> normal
            - VideoGames -> normal
        Super class (base):
        - LibraryBase -> abstract (super class)
            
        Pricing classes:    
            - FlatRatePricing -> normal
            - ProgressivePricing -> normal
        
        Services:
            - RentalService -> normal
            - LibraryService -> normal
            
    Interfaces
        - IRentable
        - IPricingStrategy
    ENUMs
        - Genre
        - MediaType
        - RegionCodes
        - ParentalGuidance 


# Methods, Fields & Properties
    IRentable:
        Properties:  
            * Id : get (string)
            * Title : get (string)
            * IsRented : get (bool)
        Methods:
            * Rent : void(string customerId)
            * Return : void()
            * CalculateFee : double(int days)

    All methods will be implemented in the classes.

# 14.10.25
## todo
    - Refactor code and add encapsulation & polymorphism
    - 