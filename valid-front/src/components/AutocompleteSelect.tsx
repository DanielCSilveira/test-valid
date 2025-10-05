import { useState, useRef, useEffect } from "react";
import "./AutocompleteSelect.css";

export interface AutocompleteOption {
  value: string;
  label: string;
}

interface AutocompleteSelectProps {
  options: AutocompleteOption[];
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  required?: boolean;
}

const AutocompleteSelect = ({
  options,
  value,
  onChange,
  placeholder = "Selecione...",
  required = false,
}: AutocompleteSelectProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [filter, setFilter] = useState("");
  const containerRef = useRef<HTMLDivElement>(null);

  const selectedOption = options.find((opt) => opt.value === value);
  const displayText = selectedOption?.label || "";

  const filteredOptions = options.filter((option) =>
    option.label.toLowerCase().includes(filter.toLowerCase())
  );

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
        setFilter("");
      }
    };

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelect = (optionValue: string) => {
    onChange(optionValue);
    setIsOpen(false);
    setFilter("");
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFilter(e.target.value);
    setIsOpen(true);
  };

  const handleInputClick = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className="autocomplete-select" ref={containerRef}>
      <input
        type="text"
        className="autocomplete-input"
        value={isOpen ? filter : displayText}
        onChange={handleInputChange}
        onClick={handleInputClick}
        placeholder={placeholder}
        required={required && !value}
        autoComplete="off"
      />
      <div className={`autocomplete-arrow ${isOpen ? "open" : ""}`}>▼</div>
      
      {isOpen && (
        <div className="autocomplete-dropdown">
          {filteredOptions.length > 0 ? (
            filteredOptions.map((option) => (
              <div
                key={option.value}
                className={`autocomplete-option ${
                  option.value === value ? "selected" : ""
                }`}
                onClick={() => handleSelect(option.value)}
              >
                {option.label}
              </div>
            ))
          ) : (
            <div className="autocomplete-empty">Nenhum resultado encontrado</div>
          )}
        </div>
      )}
    </div>
  );
};

export default AutocompleteSelect;
