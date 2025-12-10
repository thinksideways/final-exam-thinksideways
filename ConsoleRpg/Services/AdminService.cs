using ConsoleRpgEntities.Data;
using ConsoleRpgEntities.Models.Characters;
using ConsoleRpgEntities.Models.Characters.Monsters;
using ConsoleRpgEntities.Models.Abilities.PlayerAbilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using ConsoleRpgEntities.Models.Rooms;

namespace ConsoleRpg.Services;

/// <summary>
/// Handles all admin/developer CRUD operations and advanced queries
/// Separated from GameEngine to follow Single Responsibility Principle
/// </summary>
public class AdminService
{
    private readonly GameContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(GameContext context, ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Basic CRUD Operations

    /// <summary>
    /// Add a new character to the database
    /// </summary>
    public void AddCharacter()
    {
        try
        {
            _logger.LogInformation("User selected Add Character");
            AnsiConsole.MarkupLine("[yellow]=== Add New Character ===[/]");

            var name = AnsiConsole.Ask<string>("Enter character [green]name[/]:");
            var health = AnsiConsole.Ask<int>("Enter [green]health[/]:");
            var experience = AnsiConsole.Ask<int>("Enter [green]experience[/]:");

            var player = new Player
            {
                Name = name,
                Health = health,
                Experience = experience
            };

            _context.Players.Add(player);
            _context.SaveChanges();

            _logger.LogInformation("Character {Name} added to database with Id {Id}", name, player.Id);
            AnsiConsole.MarkupLine($"[green]Character '{name}' added successfully![/]");
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding character");
            AnsiConsole.MarkupLine($"[red]Error adding character: {ex.Message}[/]");
            PressAnyKey();
        }
    }

    /// <summary>
    /// Edit an existing character's properties
    /// </summary>
    public void EditCharacter()
    {
        try
        {
            _logger.LogInformation("User selected Edit Character");
            AnsiConsole.MarkupLine("[yellow]=== Edit Character ===[/]");

            var id = AnsiConsole.Ask<int>("Enter character [green]ID[/] to edit:");

            var player = _context.Players.Find(id);
            if (player == null)
            {
                _logger.LogWarning("Character with Id {Id} not found", id);
                AnsiConsole.MarkupLine($"[red]Character with ID {id} not found.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"Editing: [cyan]{player.Name}[/]");

            if (AnsiConsole.Confirm("Update name?"))
            {
                player.Name = AnsiConsole.Ask<string>("Enter new [green]name[/]:");
            }

            if (AnsiConsole.Confirm("Update health?"))
            {
                player.Health = AnsiConsole.Ask<int>("Enter new [green]health[/]:");
            }

            if (AnsiConsole.Confirm("Update experience?"))
            {
                player.Experience = AnsiConsole.Ask<int>("Enter new [green]experience[/]:");
            }

            _context.SaveChanges();

            _logger.LogInformation("Character {Name} (Id: {Id}) updated", player.Name, player.Id);
            AnsiConsole.MarkupLine($"[green]Character '{player.Name}' updated successfully![/]");
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing character");
            AnsiConsole.MarkupLine($"[red]Error editing character: {ex.Message}[/]");
            PressAnyKey();
        }
    }

    /// <summary>
    /// Display all characters in the database
    /// </summary>
    public void DisplayAllCharacters()
    {
        try
        {
            _logger.LogInformation("User selected Display All Characters");
            AnsiConsole.MarkupLine("[yellow]=== All Characters ===[/]");

            var players = _context.Players.Include(p => p.Room).ToList();

            if (!players.Any())
            {
                AnsiConsole.MarkupLine("[red]No characters found.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Health");
                table.AddColumn("Experience");
                table.AddColumn("Location");

                foreach (var player in players)
                {
                    table.AddRow(
                        player.Id.ToString(),
                        player.Name,
                        player.Health.ToString(),
                        player.Experience.ToString(),
                        player.Room?.Name ?? "Unknown"
                    );
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error displaying all characters");
            AnsiConsole.MarkupLine($"[red]Error displaying characters: {ex.Message}[/]");
        }
    }

    /// <summary>
    /// Search for characters by name
    /// </summary>
    public void SearchCharacterByName()
    {
        try
        {
            _logger.LogInformation("User selected Search Character");
            AnsiConsole.MarkupLine("[yellow]=== Search Character ===[/]");

            var searchName = AnsiConsole.Ask<string>("Enter character [green]name[/] to search:");

            var players = _context.Players
                .Include(p => p.Room)
                .Where(p => p.Name.ToLower().Contains(searchName.ToLower()))
                .ToList();

            if (!players.Any())
            {
                _logger.LogInformation("No characters found matching '{SearchName}'", searchName);
                AnsiConsole.MarkupLine($"[red]No characters found matching '{searchName}'.[/]");
            }
            else
            {
                _logger.LogInformation("Found {Count} character(s) matching '{SearchName}'", players.Count, searchName);

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Health");
                table.AddColumn("Experience");
                table.AddColumn("Location");

                foreach (var player in players)
                {
                    table.AddRow(
                        player.Id.ToString(),
                        player.Name,
                        player.Health.ToString(),
                        player.Experience.ToString(),
                        player.Room?.Name ?? "Unknown"
                    );
                }

                AnsiConsole.Write(table);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for characters");
            AnsiConsole.MarkupLine($"[red]Error searching characters: {ex.Message}[/]");
        }
    }

    #endregion

    #region C-Level Requirements

    /// <summary>
    /// DONE: Implement this method
    /// Requirements:
    /// - Display a list of existing characters [x]
    /// - Prompt user to select a character (by ID) [x]
    /// - Display a list of available abilities from the database [x]
    /// - Prompt user to select an ability to add [x]
    /// - Associate the ability with the character using the many-to-many relationship [x]
    /// - Save changes to the database [x]
    /// - Display confirmation message with the character name and ability name [x]
    /// - Log the operation [x]
    /// </summary>
    public void AddAbilityToCharacter()
    {
        _logger.LogInformation("User selected Add Ability to Character");
        AnsiConsole.MarkupLine("[yellow]=== Add Ability to Character ===[/]");

        // Display list of existing characters and prompt for ID:
        var players = _context.Players.Select(player => player);

        foreach (Player player in players.ToList())
        {
            Console.WriteLine($"Id: {player.Id}");
            Console.WriteLine($"name: {player.Name}");
        }

        Console.WriteLine("Enter player ID: ");
        var playerId = Convert.ToInt32(Console.ReadLine());

        var selectedPlayer = players.Where(player => player.Id.Equals(playerId)).FirstOrDefault();

        // Display list of existing abilities and prompt for ID:
        var abilities = _context.Abilities.Select(ability => ability);

        foreach (Ability ability in abilities.ToList())
        {
            Console.WriteLine($"Id: {ability.Id}");
            Console.WriteLine($"name: {ability.Name}");
            Console.WriteLine($"Description: {ability.AbilityType}");
        }

        Console.WriteLine($"Enter an ability ID for {selectedPlayer.Name}");
        var abilityId = Convert.ToInt32(Console.ReadLine());

        var selectedAbility = _context.Abilities.Where(ability => ability.Id.Equals(abilityId)).FirstOrDefault();

        // Update the selected player's list of abilities:
        selectedPlayer.Abilities.Add(selectedAbility);
        _context.SaveChanges();

        // Confirm and log.
        Console.WriteLine($"Player {selectedPlayer.Name} has been granted the ability: {selectedAbility.Name}");
        _logger.LogInformation($"Player {selectedPlayer.Name} (Player Id: {selectedPlayer.Id}) has been granted the ability: {selectedAbility.Name} (Ability Id: {selectedAbility.Id})");

        PressAnyKey();
    }

    /// <summary>
    /// DONE: Implement this method
    /// Requirements:
    /// - Prompt the user to select a character (by ID or name) [x]
    /// - Retrieve the character and their abilities from the database (use Include or lazy loading) [x]
    /// - Display the character's name and basic info [x]
    /// - Display all abilities associated with that character in a formatted table [x]
    /// - For each ability, show: Name, Description, and any other relevant properties (e.g., Damage, Distance for ShoveAbility) [x]
    /// - Handle the case where the character has no abilities [x]
    /// - Log the operation [x]
    /// </summary>
    public void DisplayCharacterAbilities()
    {
        _logger.LogInformation("User selected Display Character Abilities");
        AnsiConsole.MarkupLine("[yellow]=== Enter a character's ID or search by entering a name: ===[/]");

        var response = Console.ReadLine();

        int responseId = 0;
        int.TryParse(response, out responseId);

        var player = _context.Players.Where(player => player.Name.Equals(response) || player.Id.Equals(responseId)).Include(player => player.Abilities).FirstOrDefault();

        if (player is Player foundPlayer)
        {
            if (player.Abilities.Count() > 0)
            {
                // Display a table if character has abilities
                AnsiConsole.MarkupLine($"[yellow]=== {player.Abilities.ToList().Count()} abilities held by {player.Name} ({player.Id}) ===[/]");

                var table = new Table();

                table.AddColumn("Name");
                table.AddColumn("Description");
                table.AddColumn("Type");
                table.AddColumn("Damage");
                table.AddColumn("Distance");

                foreach (var ability in player.Abilities)
                {
                    table.AddRow(
                        ability.Id.ToString() ?? "N/A",
                        ability.Name ?? "N/A",
                        ability.Description ?? "N/A",
                        ability is ShoveAbility shoveAbilityDamage ? shoveAbilityDamage.Damage.ToString() : "N/A",
                        ability is ShoveAbility shoveAbilityDistance ? shoveAbilityDistance.Damage.ToString() : "N/A"
                    );
                }

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine($"[yellow]==={player.Name} ({player.Id}) has not been granted any abilities. ===[/]");
            }

            _logger.LogInformation($"User searched abilities for {player.Name} (ID: {player.Id}) and found {player.Abilities.Count()} results.");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]=== No player found with an ID or name matching {response} ===[/]");
            _logger.LogInformation($"User searched abilities with player search term: `{response}` and no players were found.");
        }
        PressAnyKey();
    }

    #endregion

    #region B-Level Requirements

    /// <summary>
    /// DONE: Implement this method
    /// Requirements:
    /// - Prompt user for room name [x]
    /// - Prompt user for room description [x]
    /// - *Optionally* prompt for navigation (which rooms connect in which directions)
    /// - Create a new Room entity [x]
    /// - Save to the database [x]
    /// - Display confirmation with room details [x]
    /// - Log the operation [x]
    /// </summary>
    public void AddRoom()
    {
        _logger.LogInformation("User selected Add Room");
        AnsiConsole.MarkupLine("[yellow]=== Add New Room ===[/]");

        var name = AnsiConsole.Ask<string>("[cyan]Enter a name for the new room: [/]");
        var description = AnsiConsole.Ask<string>("[cyan]Enter a description for the new room: [/]");

        var room = new Room();
        room.Name = name;
        room.Description = description;

        // With regard to adding room exits:
        // It didn't seem like there was any rhyme or reason to whether two rooms could possibly be the next room
        // in any cardinal direction and I definitely opted out of fixing that in any way.
        // I am aware that impossible cardinal directions are very possible and that this is a legacy issue.
        var addExit = "y";

        AnsiConsole.Ask<string>("[cyan]Would you like to add any exits? (y/n) [/]");

        if (addExit.Equals("y"))
        {
            // I made an assumption here that I could use a nullable int type into this Ask<T>()method.

            // This assumption will be prevalent in the upcoming control flow that decides
            // whether or not to add a room.

            // You can enter a huge integer while running to continue but I won't be fixing it
            // for this applciation.  It really doesn't matter to me anymore.

            // If needed for testing feel free to add a huuuuuge integer to prevent having
            // to assign an exit for each direction.
            var northId = AnsiConsole.Ask<int?>("[cyan]Enter north room ID: [/]");
            var southId = AnsiConsole.Ask<int?>("[cyan]Enter south room ID: [/]");
            var eastId = AnsiConsole.Ask<int?>("[cyan]Enter east room ID: [/]");
            var westId = AnsiConsole.Ask<int?>("[cyan]Enter west room ID: [/]");

            // add Northern exit
            if (!(northId is null) && _context.Rooms.Where(room => room.Id.Equals(northId)).Count() > 0)
            {
                room.NorthRoomId = northId;
            }
            else
            {
                AnsiConsole.MarkupLine($"There was no room with id {northId} so this exit wasn't created.");
            }

            if (!(eastId is null) && _context.Rooms.Where(room => room.Id.Equals(southId)).Count() > 0)
            {
                room.SouthRoomId = northId;
            }
            else
            {
                AnsiConsole.MarkupLine($"There was no room with id {southId} so this exit wasn't created.");
            }

            // add Eastern exit
            if (!(northId is null) && _context.Rooms.Where(room => room.Id.Equals(eastId)).Count() > 0)
            {
                room.EastRoomId = eastId;
            }
            else
            {
                AnsiConsole.MarkupLine($"There was no room with id {eastId} so this exit wasn't created.");
            }

            // add Western exit
            if (_context.Rooms.Where(room => room.Id.Equals(westId)).Count() > 0)
            {
                room.WestRoomId = westId;
            }
            else
            {
                AnsiConsole.MarkupLine($"There was no room with id {westId} so this exit wasn't created.");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("If it's not a yes it's a no.  Rooms are already impossibly next to eachother so who cares.");
        }

        _context.Rooms.Add(room);
        _context.SaveChanges();

        AnsiConsole.MarkupLine("[cyan] Room created: [/]");
        var table = new Table();

        // Special note: It's actually awesome that the entity is automatically assigned it's new ID
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");

        table.AddRow(
            room.Id.ToString(),
            room.Name,
            room.Description
        );

        AnsiConsole.Write(table);

        _logger.LogInformation($"User added room: - {room.Id} | {room.Name} | {room.Description} -");

        // DONE: Implement this method
        PressAnyKey();
    }

    /// <summary>
    /// DONE: Implement this method
    /// Requirements:
    /// - Display a list of all rooms [x]
    /// - Prompt user to select a room (by ID or name) [x]
    /// - Retrieve room from database with related data (Include Players and Monsters) [x]
    /// - Display room name, description, and exits [x]
    /// - Display list of all players in the room (or message if none) [x]
    /// - Display list of all monsters in the room (or message if none) [x]
    /// - Handle case where room is empty gracefully [x]
    /// - Log the operation [x]
    /// </summary>
    public void DisplayRoomDetails()
    {
        _logger.LogInformation("User selected Display Room Details");

        var rooms = _context.Rooms.Select(room => room).Include(room => room.Players).Include(room => room.Monsters);

        AnsiConsole.MarkupLine($"[yellow]=== Available Rooms ===[/]");

        // Create initial table of available rooms and indicate whether
        // they have players or monsters inside
        var allRoomsTable = new Table();

        allRoomsTable.AddColumn("ID");
        allRoomsTable.AddColumn("Name");
        allRoomsTable.AddColumn("Description");
        allRoomsTable.AddColumn("Exits");
        allRoomsTable.AddColumn("Has Players");
        allRoomsTable.AddColumn("Has Monsters");

        foreach (var room in rooms.ToList())
        {
            var exits = new List<string>();

            if (room.NorthRoomId != null) exits.Add("North");
            if (room.SouthRoomId != null) exits.Add("South");
            if (room.EastRoomId != null) exits.Add("East");
            if (room.WestRoomId != null) exits.Add("West");

            allRoomsTable.AddRow(
                room.Id.ToString() ?? "N/A",
                room.Name ?? "N/A",
                room.Description ?? "N/A",
                string.Join(",", exits),
                room.Players.Count() > 0 ? "TRUE" : "FALSE",
                room.Monsters.Count() > 0 ? "TRUE" : "FALSE"
            );
        }

        AnsiConsole.Write(allRoomsTable);

        AnsiConsole.MarkupLine("[yellow]=== Display Room Details ===[/]");
        var response = AnsiConsole.Ask<string>("[yellow] Enter the room ID or the room name: [/]");
        int responseId = 0;
        int.TryParse(response, out responseId);

        var foundRoom = _context.Rooms.Where(room => room.Name.Equals(response) || room.Id.Equals(responseId)).Include(room => room.Players).Include(room => room.Monsters).FirstOrDefault();

        if (foundRoom is Room found)
        {
            var foundRoomTable = new Table();

            foundRoomTable.AddColumn("ID");
            foundRoomTable.AddColumn("Name");
            foundRoomTable.AddColumn("Description");
            foundRoomTable.AddColumn("Exits");
            foundRoomTable.AddColumn("Players");
            foundRoomTable.AddColumn("Monsters");

            // Summarized string of players and monsters for tabular cell output
            var playerString = "There are no players in this room.";
            if (foundRoom.Players.Count() > 0)
            {
                playerString = "";
                foreach (Player player in foundRoom.Players)
                {
                    playerString += $"{player.Id}. {player.Name} ";
                }
            }

            var monsterString = "There are no monsters in this room.";
            if (foundRoom.Monsters.Count() > 0)
            {
                monsterString = "";
                foreach (Monster monster in foundRoom.Monsters)
                {
                    monsterString += $"{monster.Id}. {monster.Name} ";
                }
            }

            List<string> exits = new List<string>();

            if (foundRoom.NorthRoomId != null) exits.Add("North");
            if (foundRoom.SouthRoomId != null) exits.Add("South");
            if (foundRoom.EastRoomId != null) exits.Add("East");
            if (foundRoom.WestRoomId != null) exits.Add("West");

            foundRoomTable.AddRow(
                foundRoom.Id.ToString() ?? "N/A",
                foundRoom.Name ?? "N/A",
                foundRoom.Description ?? "N/A",
                string.Join(",", exits),
                playerString,
                monsterString
            );

            AnsiConsole.Write(foundRoomTable);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]=== No rooms found ===[/]");
        }

        _logger.LogInformation($"User searched rooms.");
        // DONE: Implement this method
        PressAnyKey();
    }

    #endregion

    #region A-Level Requirements

    /// <summary>
    /// TODO: Implement this method
    /// Requirements:
    /// - Display list of all rooms
    /// - Prompt user to select a room
    /// - Display a menu of attributes to filter by (Health, Name, Experience, etc.)
    /// - Prompt user for filter criteria
    /// - Query the database for characters in that room matching the criteria
    /// - Display matching characters with relevant details in a formatted table
    /// - Handle case where no characters match
    /// - Log the operation
    /// </summary>
    public void ListCharactersInRoomByAttribute()
    {
        _logger.LogInformation("User selected List Characters in Room by Attribute");
        AnsiConsole.MarkupLine("[yellow]=== List Characters in Room by Attribute ===[/]");

        // TODO: Implement this method
        AnsiConsole.MarkupLine("[red]This feature is not yet implemented.[/]");
        AnsiConsole.MarkupLine("[yellow]TODO: Find characters in a room matching specific criteria.[/]");

        PressAnyKey();
    }

    /// <summary>
    /// TODO: Implement this method
    /// Requirements:
    /// - Query database for all rooms
    /// - For each room, retrieve all characters (Players) in that room
    /// - Display in a formatted list grouped by room
    /// - Show room name and description
    /// - Under each room, list all characters with their details
    /// - Handle rooms with no characters gracefully
    /// - Consider using Spectre.Console panels or tables for nice formatting
    /// - Log the operation
    /// </summary>
    public void ListAllRoomsWithCharacters()
    {
        _logger.LogInformation("User selected List All Rooms with Characters");
        AnsiConsole.MarkupLine("[yellow]=== List All Rooms with Characters ===[/]");

        // TODO: Implement this method
        AnsiConsole.MarkupLine("[red]This feature is not yet implemented.[/]");
        AnsiConsole.MarkupLine("[yellow]TODO: Group and display all characters by their rooms.[/]");

        PressAnyKey();
    }

    /// <summary>
    /// TODO: Implement this method
    /// Requirements:
    /// - Prompt user for equipment/item name to search for
    /// - Query the database to find which character has this equipment
    /// - Use Include to load Equipment -> Weapon/Armor -> Item
    /// - Also load the character's Room information
    /// - Display the character's name who has the equipment
    /// - Display the room/location where the character is located
    /// - Handle case where equipment is not found
    /// - Handle case where equipment exists but isn't equipped by anyone
    /// - Use Spectre.Console for nice formatting
    /// - Log the operation
    /// </summary>
    public void FindEquipmentLocation()
    {
        _logger.LogInformation("User selected Find Equipment Location");
        AnsiConsole.MarkupLine("[yellow]=== Find Equipment Location ===[/]");

        // TODO: Implement this method
        AnsiConsole.MarkupLine("[red]This feature is not yet implemented.[/]");
        AnsiConsole.MarkupLine("[yellow]TODO: Find which character has specific equipment and where they are located.[/]");

        PressAnyKey();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method for user interaction consistency
    /// </summary>
    private void PressAnyKey()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    #endregion
}
