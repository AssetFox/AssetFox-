export const convertBase64ToArrayBuffer = (base64String: string): ArrayBuffer => {

  const binaryString = window.atob(base64String);
  const binaryStringLength = binaryString.length;
  const bytes = new Uint8Array(binaryStringLength);
  for (let i = 0; i < binaryStringLength; i++) {
    bytes[i] = binaryString.charCodeAt(i);
  }

  return bytes.buffer as ArrayBuffer;
};