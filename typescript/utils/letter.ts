import {ERRORS} from "../constants";

export const retrieveWord = (words: string | any[]) => words[Math.ceil(words.length - 1)];

export const clearUnmaskedWord = (game: any) => {
    const withoutUnmasked = {
        ...game,
    };
    delete withoutUnmasked.unmaskedWord;
    return withoutUnmasked;
};

export function findLetterPositions(word: string, letter: string): { positions: number[], found: boolean } {
    // Validate the letter
    if (!/^[a-zA-Z]$/.test(letter)) {
        throw new Error(ERRORS.SINGLE_ALPHABETIC_CHARACTER);
    }

    // Validate the word
    if (!/^[a-zA-Z]{2,}$/.test(word)) {
        throw new Error(ERRORS.WORD_VALIDATION);
    }

    const positions: number[] = [];
    const lowerCaseWord = word.toLowerCase();
    const lowerCaseLetter = letter.toLowerCase();

    for (let i = 0; i < lowerCaseWord.length; i++) {
        if (lowerCaseWord[i] === lowerCaseLetter) {
            positions.push(i);
        }
    }

    return {
        positions,
        found: positions.length > 0
    };
}