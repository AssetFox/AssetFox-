export const convertBase64ToArrayBuffer = (base64String: string): ArrayBuffer => {
  try {
    // Sanitize the Base64 string
    let sanitizedBase64 = base64String
      .replace(/-/g, '+') // Replace URL-safe characters
      .replace(/_/g, '/')
      .replace(/\s/g, ''); // Remove whitespace

    // Add padding if necessary
    const padding = sanitizedBase64.length % 4;
    if (padding > 0) {
      sanitizedBase64 += '='.repeat(4 - padding);
    }

    // Decode the Base64 string
    const binaryString = window.atob(sanitizedBase64);

    // Convert binary string to Uint8Array
    const bytes = Uint8Array.from(binaryString, (char) => char.charCodeAt(0));

    return bytes.buffer as ArrayBuffer;
  } catch (error) {
    console.error('Failed to decode Base64 string:', error);
    throw new Error('Invalid Base64 string');
  }
};
