export type Dictionary<TKey extends number | string, TValue> = { readonly [key in TKey]: TValue };

export class KeyValuePair<TKey extends number | string | boolean, TValue> {

    constructor(
        readonly key: TKey,
        readonly value: TValue
    ) {
    }

    static fromDictionary<TKey extends number | string, TValue>(dictionary: Dictionary<TKey, TValue>, keyType: 'string' | 'number') {
        const isNumber = keyType === 'number';
        const result = new Array<KeyValuePair<TKey, TValue>>();

        for (const key of Object.keys(dictionary)) {
            const keyValue = isNumber
                ? +key
                : key;
            const value = dictionary[keyValue as TKey];
            const pair = new KeyValuePair(keyValue as TKey, value);

            result.push(pair);
        }

        return result;
    }

}
