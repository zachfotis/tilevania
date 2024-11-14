#!/bin/bash

# Check if size threshold is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 [size_threshold_in_MB]"
    exit 1
fi

size_threshold_mb=$1
size_threshold_bytes=$(echo "$size_threshold_mb * 1024 * 1024" | bc)
size_threshold_bytes=${size_threshold_bytes%.*}

echo "Scanning for files larger than $size_threshold_mb MB (excluding files in .gitignore)..."

# Get a list of all files not ignored by Git
all_files=$(git ls-files --others --cached --exclude-standard)

if [ -z "$all_files" ]; then
    echo "No files to scan."
    exit 0
fi

extensions=()
found_large_files=false

while IFS= read -r filepath; do
    # Check if the file exists (it might have been deleted)
    if [ ! -f "$filepath" ]; then
        continue
    fi

    # Use 'stat -f%z' on macOS, 'stat -c%s' on Linux
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        filesize=$(stat -f%z "$filepath")
    else
        # Linux
        filesize=$(stat -c%s "$filepath")
    fi

    if [ "$filesize" -ge "$size_threshold_bytes" ]; then
        found_large_files=true
        filename=$(basename "$filepath")
        human_readable_size=$(du -h "$filepath" | cut -f1)
        echo "Found large file: $filepath ($human_readable_size)"

        # Get the file extension
        extension="${filename##*.}"
        if [ "$extension" != "$filename" ]; then
            # Check if the extension is already in the array
            if [[ ! " ${extensions[@]} " =~ " $extension " ]]; then
                extensions+=("$extension")
            fi
        fi
    fi
done <<< "$all_files"

if [ "$found_large_files" = false ]; then
    echo "No files exceed the specified size threshold."
    exit 0
fi

# Configure Git LFS to track files with the identified extensions
for ext in "${extensions[@]}"; do
    pattern="*.$ext"
    git lfs track "$pattern"
    echo "Tracking $pattern with Git LFS."
done

# Add .gitattributes to Git
git add .gitattributes
echo
echo "Updated .gitattributes with Git LFS tracking patterns."
echo "Don't forget to commit the changes:"
echo '    git commit -m "Configure Git LFS for large files"'
