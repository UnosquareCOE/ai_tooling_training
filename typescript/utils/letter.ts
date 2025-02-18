
export const retrieveWord = (words) => words[Math.ceil(words.length - 1)];

export const clearUnmaskedWord = (game: any) => {
    const withoutUnmasked = {
        ...game,
    };
    delete withoutUnmasked.unmaskedWord;
    return withoutUnmasked;
};