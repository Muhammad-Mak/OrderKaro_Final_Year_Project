// getOrdinalDate.ts
// This utility function takes a date string as input and returns a human-readable date string
// with an ordinal suffix (e.g., "1st Jan", "2nd Feb", "3rd Mar", etc.).

export function getOrdinalDate(dateStr: string): string {
  const date = new Date(dateStr); // Convert the input string to a Date object
  const day = date.getDate(); // Extract the day of the month (1–31)
  const month = date.toLocaleString('en-GB', { month: 'short' }); // Get abbreviated month name (e.g., Jan, Feb)

  // Helper function to determine the correct ordinal suffix
  const suffix = (n: number): string => {
    // Special case for numbers ending in 11, 12, 13 which all use 'th'
    if (n >= 11 && n <= 13) return 'th';
    
    // Determine suffix based on the last digit
    switch (n % 10) {
      case 1: return 'st'; // 1st
      case 2: return 'nd'; // 2nd
      case 3: return 'rd'; // 3rd
      default: return 'th'; // All other cases
    }
  };

  // Return formatted string like "21st Jul"
  return `${day}${suffix(day)} ${month}`;
}
