export const convertBase64ToArrayBuffer = (base64String: string): ArrayBuffer => {

  const binaryString = Buffer.from(base64String, 'base64');

  const bytes = new Uint8Array(binaryString.length);

  for (let i = 0; i < binaryString.length; i++) {

    bytes[i] = binaryString[i];

  }

  return bytes.buffer as ArrayBuffer;

};