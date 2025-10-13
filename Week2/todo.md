# TODO 13.10.2025
    ## 

# Application
    ## Library application
        - Help the library keep track of which books are rented out, and which ones are not.
 

# Classes, Interfaces & ENUM
    
    Classes (marked: normal, static, abstract, internal)
        - Book -> normal
        - LibraryBase -> abstract (super class)
        - FlatRatePricing -> normal
        - ProgressivePricing -> normal
        - RentalService -> normal
    Interfaces
        - IRentable
        - IPricingStrategy
    ENUMs
        - Genre


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

    All methods will be implemented in these classes.

# OOP concepts