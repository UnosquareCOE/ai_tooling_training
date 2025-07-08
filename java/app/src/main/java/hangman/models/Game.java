package hangman.models;

import com.google.gson.annotations.Expose;

import java.util.ArrayList;
import java.util.List;

public class Game {
    @Expose
    private int remainingGuesses;
    @Expose
    private String word;
    @Expose(serialize = false, deserialize = false)
    private String unmaskedWord;
    @Expose
    private List<String> incorrectGuesses;
    @Expose
    private String status;

    public Game(int guesses, String selectedWord) {
        remainingGuesses = guesses;
        unmaskedWord = selectedWord;
        word = selectedWord.replaceAll("[a-zA-Z0-9_]", "_");
        status = "In Progress";
        incorrectGuesses = new ArrayList<>();
    }

    // Getters and Setters
    public int getRemainingGuesses() {
        return remainingGuesses;
    }

    public void setRemainingGuesses(int remainingGuesses) {
        this.remainingGuesses = remainingGuesses;
    }

    public String getWord() {
        return word;
    }

    public void setWord(String word) {
        this.word = word;
    }

    public String getUnmaskedWord() {
        return unmaskedWord;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public List<String> getIncorrectGuesses() {
        return incorrectGuesses;
    }
}
