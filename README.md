# Hashcat Scheduler

With **hashcat-scheduler**, it allows you to use **hashcat** with predefined attack scenarios. You simply need to provide the required scenarios, and **hashcat-scheduler** will execute them one by one until the password is cracked or all scenarios are exhausted without finding the password.

## Installation

* Download lastest version from [here!](https://github.com/dong-nguyen-hd/hashcat-scheduler/releases/tag/v1.0.0)

## Usage
The hashcat-scheduler contains 4 folders, namely:
* **dictionaries:** contains dictionary data and the file *mapping.json*. You need to modify the information in the *mapping.json* file to match the number of dictionaries you have. Note that each dictionary is represented by a number in the *mapping.json* file.
* **directions:** contains information about the scenarios you plan to use, all of which are stored in the *directions.json* file. Note that these scenarios are numbered according to the order of execution, arranged in ascending order.
* **hashcat:** contains the executable file of *hashcat*.
* **input:** contains the passwords to be decrypted.


## License

This project is licensed with the [MIT license](LICENSE).