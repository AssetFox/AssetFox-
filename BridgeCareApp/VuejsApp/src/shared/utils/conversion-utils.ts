export const mapToIndexSignature = <T>(map: Map<string, T>): {[key: string]: T} =>{
    let toReturn: {[key: string]: T} = {}

    Array.from(map.keys()).forEach(key => toReturn[key] = map.get(key)!)

    return toReturn;
}